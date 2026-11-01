# TOOLS 索引

本目录收自写脚本与随项目落地的工具。一级子目录只有 `scripts/`（自写脚本统一收纳）；第三方开发环境（.NET SDK、Godot、Python）装在系统目录，不复制进项目。

## 工具清单

| 时间 | 工具/脚本 | 用途 | 版本 | 大小 | 来源 | 安装方式/依赖 |
|---|---|---|---|---|---|---|
| 2026-08-28 17:48 | [scripts/pck_list.py](scripts/pck_list.py) | 解析 Godot 4 `pack format 3` 的 `.pck`：列出文件表或按关键字提取（只读，不写游戏目录） | 无版本号（一次性脚本） | 2.0 KB | 自写 | Python 3 标准库；直接运行 `python pck_list.py [list\|extract] <关键字> [输出目录]` |
| 2026-08-28 17:52 | [scripts/ctex_to_png.py](scripts/ctex_to_png.py) | 从 Godot `.ctex` 中找出内嵌 WebP、取面积最大的块转存 PNG | 无版本号（一次性脚本） | 1.1 KB | 自写 | Python 3 + Pillow（`pip install pillow`）；`python ctex_to_png.py <in.ctex> <out.png>` |

## 系统环境（未纳入本目录）

| 名称 | 用途 | 说明 |
|---|---|---|
| .NET SDK（net9.0） | 编译 `9.SRC/DarvRelicPoolMod/` | 由 `dotnet --version` 检测，工程目标框架 `net9.0` |
| Godot 4.5.1（.NET 版） | 导入资源、导出 pck | 路径见 `9.SRC/DarvRelicPoolMod/build.ps1` |
| Python 3 + Pillow | 跑上面两个脚本 | 仅封面/解包工作使用，不参与 mod 构建 |
| GitHub CLI（`gh`） | 创建 Release、上传资产 | 已登录 `Rim-World` |

上述工具的具体安装位置以本机环境为准；本项目不重复安装。
