namespace FileService.Data
{
    public class Files : FilesBase
    {
        public Files() : base("fs.files") { }
    }
    public class FilesConvert : FilesBase
    {
        public FilesConvert() : base("convert.files") { }
    }
}
