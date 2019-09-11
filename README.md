# Devtools
just a collection of tools for development

# GitIgnore
This tool can be used to create `.gitignore` files from commandline.
Just copy the tool into a directory included in your environment path.
The tool searches a directory for specific ignore files based on a search pattern.
The directory to search must be set using the `config` verb. `ignore config -d <path-to-directory>`

``` bash
cd <your-repo-folder>
git clone gitignore https://github.com/github/gitignore.git
cd gitignore
ignore config --directory .

cd <your-project-folder>
ignore select -p visual*
```

## commandline interface
  * `config`     gets or sets program options
    * `-d, --directory`  (Default: ./) sets the source directory
    * `-s, --show`       (Default: false) displays the configuration
  * `list`       displays a list of possible git ignore settings.
    * `-p, --pattern`    (Default: *.gitignore) an optional search pattern.
  * `select`     creates the desired ignore file in the current directory.
    * `-p, --pattern`    Required. The search pattern to find the desired ignore file.
    * `-f, --force`      (Default: false) The selection will override any existing ignore file when this switch is set.
  * `help`       Display more information on a specific command.
  * `version`    Display version information.
