"""解析 Godot 4 pack format 3 pck，列出/提取指定文件。只读 pck，不写游戏目录。"""
import struct, sys, os

PCK = r"C:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck"

def read_dir(f):
    f.seek(0)
    assert f.read(4) == b"GDPC"
    (pack_ver,) = struct.unpack("<I", f.read(4))
    f.read(12)          # engine major/minor/patch
    (flags,) = struct.unpack("<I", f.read(4))
    (file_base,) = struct.unpack("<Q", f.read(8))
    (dir_off,) = struct.unpack("<Q", f.read(8))
    f.seek(dir_off)
    (n,) = struct.unpack("<I", f.read(4))
    entries = []
    for _ in range(n):
        (plen,) = struct.unpack("<I", f.read(4))
        path = f.read(plen).split(b"\x00", 1)[0].decode("utf-8", "replace")
        ofs, size = struct.unpack("<QQ", f.read(16))
        f.read(16)      # md5
        (eflags,) = struct.unpack("<I", f.read(4))
        entries.append((path, file_base + ofs, size, eflags))
    return pack_ver, flags, n, entries

if __name__ == "__main__":
    mode = sys.argv[1] if len(sys.argv) > 1 else "list"
    pat = sys.argv[2].lower() if len(sys.argv) > 2 else "darv"
    with open(PCK, "rb") as f:
        pv, flags, n, entries = read_dir(f)
        print(f"# pack_ver={pv} pack_flags={flags} files={n} matched={0}")
        matches = [e for e in entries if pat in e[0].lower()]
        print(f"# matched {len(matches)}")
        for path, off, size, efl in matches:
            print(f"{path}  off={off} size={size} flags={efl}")
        if mode == "extract":
            outdir = sys.argv[3]
            for path, off, size, efl in matches:
                rel = path.lstrip("/").replace("res://", "")
                dst = os.path.join(outdir, rel.replace("/", os.sep))
                os.makedirs(os.path.dirname(dst), exist_ok=True)
                f.seek(off)
                data = f.read(size)
                with open(dst, "wb") as w:
                    w.write(data)
                print(f"EXTRACTED -> {dst} ({size} bytes)")
