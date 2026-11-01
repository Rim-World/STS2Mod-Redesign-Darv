# TMP 索引

本目录存放临时与过程文件，内容**不入库**（`.gitignore` 只放行本文件）。一级子目录按分类划分，每类一个文件夹。

## 分类与内容

| 时间 | 文件/目录 | 用途 | 格式 | 大小 | 来源 |
|---|---|---|---|---|---|
| 2026-08-29 03:00 | `02-缓存与临时/fonts/ZCOOLQingKeHuangYou-Regular.ttf` | 封面制作用的站酷庆科黄油体（子集） | TTF | 26 KB | 站酷字体，本机字体库 |
| 2026-08-29 05:50 | `02-缓存与临时/fonts/ZCOOLQingKeHuangYou-Full.ttf` | 同上，全字符版（子集化的输入） | TTF | 8.0 MB | 站酷字体 |
| 2026-08-29 05:48 | `02-缓存与临时/fonts/ZCOOL-subset-cover3.ttf` | 封面文案专用子集 | TTF | 11 KB | 由全字库子集化生成 |
| 2026-08-29 05:44 | `02-缓存与临时/fonts/ZCOOL-subset-huigui.ttf` | 另一版封面文案子集 | TTF | 12 KB | 同上 |
| 2026-08-29 03:01 | `02-缓存与临时/fonts/ZCOOL-subset.ttf` | 首版文案子集 | TTF | 9 KB | 同上 |
| 2026-08-29 03:01 | `02-缓存与临时/fonts/subset_url.txt` | 记录子集化所用在线服务的 URL | 纯文本 | 137 B | 手工记录 |
| 2026-08-28 17:48 | `02-缓存与临时/代码片段/file_access_pack.cpp` | Godot 引擎 `FileAccessPack` 源码片段，用于核对 pck format 3 的头部/目录结构 | C++ | 18 KB | Godot 引擎开源代码 |
| 2026-08-28 17:49 | `02-缓存与临时/pck_out/` | `SlayTheSpire2.pck` 的解包输出（`.import`、`.tres`、`.tscn`、少量 `.cs`），用于定位达弗事件 CG、纪元立绘与图集路径 | Godot 资源文本 | 2.4 MB / 54 文件 | 由 `TOOLS/scripts/pck_list.py` 解包 |
| 2026-08-28 17:52 | `02-缓存与临时/图片/darv_epoch.png` | 达弗纪元立绘的中间产物 | PNG | 512 KB | 由 `.ctex` 提取 |
| 2026-08-28 17:52 | `02-缓存与临时/图片/darv_event_cg.png` | 达弗事件 CG 的中间产物（成品见 `pics/darv.png`） | PNG | 2.6 MB | 同上 |
| — | `02-缓存与临时/tmp-pck-extract/` | 原 `research/tmp-pck-extract/`，解包过程中的空目录 | 目录 | 0 | 历史遗留 |

## 说明

- `01-旧Git仓库/`：本项目没有嵌套的克隆仓库（`9.SRC/DarvRelicPoolMod/` 下无 `.git`），故未使用。
- `03-运行日志/`：当前没有构建日志或运行日志需要归档。
- `02-缓存与临时/图片/` 中的两张 PNG 与 `pics/` 中的成品重复，保留的是可再生成的过程文件。

## 清理建议

| 对象 | 建议 | 理由 |
|---|---|---|
| `02-缓存与临时/pck_out/`、`tmp-pck-extract/` | 可清理 | 由 `TOOLS/scripts/pck_list.py` 随时可再生成 |
| `02-缓存与临时/图片/` | 可清理 | 成品已在 `pics/` |
| `02-缓存与临时/fonts/` | 保留 | 8 MB 字体为封面重制所需；若不再改封面可清理 |
| `02-缓存与临时/代码片段/file_access_pack.cpp` | 保留 | 体积小，解包方法的知识来源 |

合计 14 MB / 63 个文件。
