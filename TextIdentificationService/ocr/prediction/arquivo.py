import os

for (dirpath, dirnames, filenames) in os.walk(r'C:\Projetos\Mestrado\Project II\SourceCode\TextIdentificationService\datasets\a_z_font'):
        for filename in filenames:
                if(filename.__contains__("font_font_")):
                        os.rename(os.path.join(dirpath, filename), os.path.join(dirpath, filename[5:]))
