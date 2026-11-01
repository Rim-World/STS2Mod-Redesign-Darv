"""从 Godot .ctex 中提取内嵌 WebP 并转存 PNG。"""
import sys, re
from PIL import Image

def extract(ctex_path, out_png):
    with open(ctex_path, "rb") as f:
        data = f.read()
    results = []
    for m in re.finditer(b"RIFF", data):
        o = m.start()
        if o + 12 > len(data) or data[o+8:o+12] != b"WEBP":
            continue
        size = int.from_bytes(data[o+4:o+8], "little")
        if o + 8 + size > len(data):
            continue
        webp = data[o:o+8+size]
        try:
            img = Image.open(__import__("io").BytesIO(webp))
            img.load()
            results.append((o, size, img.size, img))
        except Exception as e:
            print(f"  block@{o}: decode failed: {e}")
    for o, size, sz, _ in results:
        print(f"  block@{o}: webp={size}B image={sz[0]}x{sz[1]}")
    if not results:
        raise SystemExit("no decodable WebP found")
    best = max(results, key=lambda r: r[2][0] * r[2][1])
    best[3].save(out_png, "PNG")
    print(f"SAVED {out_png} ({best[2][0]}x{best[2][1]})")

if __name__ == "__main__":
    extract(sys.argv[1], sys.argv[2])
