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
| 2026-11-01 19:50 | `01-旧Git仓库/remote-probe.git` | 整理期用于探查远端历史的临时克隆（`git init` + `git fetch origin --tags`），只读用途，非嵌套源码仓库 | Git 仓库 | 287 KB | 仓库整理时创建 |
| 2026-11-01 20:03 | `02-缓存与临时/校验记录/` | blob 保全校验用的 oid 清单（`old_oids.txt` / `new_oids.txt` / `remote_new_oids.txt`）、整理提交信息（`commitmsg.txt`）、v1.0.0 发布说明（`release-notes.md`） | 纯文本 / Markdown | 28 KB | 仓库整理时生成 |
| 2026-11-01 19:50 | `02-缓存与临时/远端根文件备份/` | 恢复前从远端 `main` 取出的 `.gitignore`、`LICENSE`、`README.md`、`README.zh.md` 副本，供对照 | 文本 / Markdown | 14 KB | 远端 `bb5aeab` |

## 说明

- `01-旧Git仓库/`：本项目 `9.SRC/DarvRelicPoolMod/` 下没有嵌套克隆，本目录只放整理期使用的远端探查克隆。
- `03-运行日志/`：当前没有构建日志或运行日志需要归档。
- `02-缓存与临时/图片/` 中的两张 PNG 与 `pics/` 中的成品重复，保留的是可再生成的过程文件。

## 清理建议

| 对象 | 建议 | 理由 |
|---|---|---|
| `02-缓存与临时/pck_out/`、`tmp-pck-extract/` | 可清理 | 由 `TOOLS/scripts/pck_list.py` 随时可再生成 |
| `02-缓存与临时/图片/` | 可清理 | 成品已在 `pics/` |
| `02-缓存与临时/fonts/` | 保留 | 8 MB 字体为封面重制所需；若不再改封面可清理 |
| `02-缓存与临时/代码片段/file_access_pack.cpp`、`02-缓存与临时/校验记录/`、`01-旧Git仓库/remote-probe.git` | 保留 | 体积小，是整理过程与解包方法的证据 |

合计 14 MB / 99 个文件。

## 2026-11-01 清理记录

- `TMP/01-旧Git仓库/` 与 `TMP/02-缓存与临时/` 的内容已全部移入 Windows 回收站。
- 清理依据：远端仓库已完整保留历史，构建缓存可再生；需要回滚时从远端 `git log` 或 `10.OUTPUT/` 取回。
