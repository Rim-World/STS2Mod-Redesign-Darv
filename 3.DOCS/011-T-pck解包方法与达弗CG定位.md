# 011-T pck 解包方法与达弗 CG 定位

> 2026-08-28 沉淀。目的：解包原版达弗 CG 到 `pics/darv.png`。方法可复用于解包任意原版资源。

## 游戏与包体

- 游戏：`C:\Game\Steam\steamapps\common\Slay the Spire 2`，release_info.json 显示 **v0.111.0**（commit 41cef1ea）。
- 资源包：`SlayTheSpire2.pck`（约 1.99 GB），**Godot 4.5.1 / pack format 3**，共 15,890 个文件。
- pck 未加密（pack_flags=2，仅 PACK_REL_FILEBASE；无 PACK_DIR_ENCRYPTED）。

## pck format 3 目录结构（Godot 4.5+，区别于 format 1/2）

```
头部:
  0x00  "GDPC"
  0x04  u32 pack_format_version = 3
  0x08  u32 ×3 引擎版本 major/minor/patch（4.5.1）
  0x14  u32 pack_flags（bit0=目录加密，bit1=偏移相对 file_base；V3 恒启用相对偏移）
  0x18  u64 file_base = 0x70（文件数据区起点）
  0x20  u64 dir_offset = 0x76906ba0（目录在文件尾部！）
  0x28  直接 seek 到 dir_offset
目录:
  u32 file_count
  每项: u32 path_len; utf8 path(含\0); u64 offset(相对 file_base); u64 size; 16B md5; u32 flags(bit0=单文件加密)
实际数据位置 = pck_start_pos + file_base + offset
```

参考：`godot/4.5/core/io/file_access_pack.cpp`（已存 `TMP/02-缓存与临时/代码片段/file_access_pack.cpp`）；参考资料库自带示例 `STS2-Reference/2.STS2-111Reference/extract-sts2-card-reference.py`。

## .ctex 提取（Godot 导入纹理容器）

原版 `.png` 在 pck 中均为 `.godot/imported/<name>.png-<md5>.ctex`（`.import` 文件在原路径占位）。

```
ctex 头 56 字节: "GST2" + u16 version=1 + u16 0 + u32 width + u32 height + u32 flags(0x0d)
  + i32 -1 + 16B 0 + u32 data_format(2=WebP) + u16 width + u16 height + u32 mipmaps + u32 format
56 字节起: 直接是 WebP 数据（"RIFF"...."WEBP"），RIFF 块长度字段给出实际大小
```

- 用 Pillow（需 webp 支持）找到 `RIFF...WEBP` 块即可无损解码出原图。
- 脚本：`TOOLS/scripts/ctex_to_png.py`、pck 列目录/提取脚本：`TOOLS/scripts/pck_list.py`。

## 达弗（darv）资源路径清单（pck 内）

| 用途 | pck 内路径 | 尺寸 | 说明 |
|---|---|---|---|
| **事件 CG** | `res://images/ancients/darv_placeholder.png` | 2560×1200 | `scenes/events/background_scenes/darv.tscn` 整屏 TextureRect 引用；文件名叫 placeholder 但就是正式 CG |
| 纪元立绘 | `res://images/timeline/epoch_portraits/darv_epoch.png` | 810×500 | 时间线纪元界面（`EpochModel.RealPortrait`） |
| 地图节点图标 | `res://images/packed/map/ancients/ancient_node_darv.png`(+`_outline`) | 小 | 先古之民地图点 |
| 运行记录图标 | `res://images/ui/run_history/darv.png`(+`_outline`) | 小 | 局内记录 |
| 事件场景 | `res://scenes/events/background_scenes/darv.tscn` | - | 含星光粒子等，无 Spine/骨骼动画 |

代码依据（decompiled-111）：`MegaCrit.Sts2.Core.Models.EventModel.cs:198`（普通事件立绘规则 `images/events/<id>.png`，达弗目录下无此文件，先古之民走背景场景）；`MegaCrit.Sts2.Core.Timeline.EpochModel.cs:185`。

## 本次产出

- `pics/darv.png` — 2560×1200 事件 CG（PNG，2.68 MB）
- `pics/darv_epoch.png` — 810×500 纪元立绘（PNG，524 KB）
- 过程文件均在 `TMP/02-缓存与临时/pck_out/`（未动游戏目录与已有文件）
