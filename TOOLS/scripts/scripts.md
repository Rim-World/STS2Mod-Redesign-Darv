# scripts 索引

本目录收自写的一次性脚本，均与 mod 构建无关，只在素材制作/资源解包时使用。

| 时间 | 脚本 | 用途 | 依赖 | 用法 |
|---|---|---|---|---|
| 2026-08-28 | `pck_list.py` | 解析 `SlayTheSpire2.pck`（Godot 4 pack format 3）的文件表，按关键字列出条目或提取文件到指定目录 | Python 3 标准库（`struct`/`sys`/`os`） | `python pck_list.py list darv`<br>`python pck_list.py extract darv <输出目录>` |
| 2026-08-28 | `ctex_to_png.py` | 从 Godot `.ctex`（纹理缓存）中提取内嵌 WebP，取面积最大的一块存为 PNG | Python 3 + Pillow | `python ctex_to_png.py <输入.ctex> <输出.png>` |

## 注意

- `pck_list.py` 顶部的 `PCK` 常量写死了本机游戏路径 `C:\Game\Steam\steamapps\common\Slay the Spire 2\SlayTheSpire2.pck`，换机器需改这一行。
- 两个脚本对游戏目录只读；提取产物写入调用者指定的目录。
- 方法说明见 `3.DOCS/011-T-pck解包方法与达弗CG定位.md`。
