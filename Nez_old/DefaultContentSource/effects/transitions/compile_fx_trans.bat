@echo off
set "respath=C:\Users\Creme\Desktop\LF\Cookie Dough\Cookie Dough\bin\Windows\AnyCPU\Debug\Content\nez\effects\transitions"
cd "C:\Users\Creme\source\repos\Nez\Nez\DefaultContentSource\effects\transitions"
for %%f in (*.fx) do (
    mgfxc "%%~nf.fx" "%respath%\%%~nf.mgfxo" /Profile:OpenGL
)
pause