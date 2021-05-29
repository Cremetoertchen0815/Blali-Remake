@echo off
set "respath=C:\Users\Creme\source\repos\NezTestVB\NezTestVB\bin\Windows\AnyCPU\Debug\Content\nez\effects"
cd "C:\Users\Creme\source\repos\Nez\Nez\DefaultContentSource\effects"
for %%f in (*.fx) do (
    mgfxc "%%~nf.fx" "%respath%\%%~nf.mgfxo" /Profile:OpenGL
)
pause