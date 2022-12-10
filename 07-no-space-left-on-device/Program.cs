namespace no_space_left_on_device
{
    class DiskFile
    {

        public DiskFile(string name, int size, DiskFolder parent)
        {
            Name = name;
            Size = size;
            Parent = parent;
        }

        public string Name { get; set; } = string.Empty;
        public int Size { get; set; }

        public DiskFolder Parent;
    }

    class DiskFolder
    {
        public string Name { get; set; } = string.Empty;
        public string Path
        {
            get
            {
                if (Parent == null) return "/";
                return Parent.Path + Name + "/";
            }
        }
        private int? _size = null;
        public int Size
        {
            get
            {
                if (_size.HasValue)
                    return _size.Value;

                _size = Files.Select(file => file.Size).Sum() + 
                    Folders.Select(folder => folder.Size).Sum();

                return _size.Value;
            }
        }

        public DiskFolder? Parent { get; set; }
        public List<DiskFolder> Folders { get; set; } = new List<DiskFolder>();
        public List<DiskFile> Files { get; set; } = new List<DiskFile>();

        public DiskFolder(string name, DiskFolder? parent)
        {
            Name = name;
            Parent = parent;
        }

        public DiskFolder Navigate(string dir)
        {
            if (dir == "..")
                return Parent ?? this;

            if(dir == "/")
            {
                if (Parent == null) return this;
                return Parent.Navigate(dir);
            }
                
            var folder = Folders.FirstOrDefault(z => z.Name == dir);
            if (folder != null)
                return folder;

            folder = new DiskFolder(dir, this);
            return folder;
        }

        public DiskFile AddFile(string name, int size)
        {
            DiskFile f = new DiskFile(name, size, this);
            Files.Add(f);

            _size = null;
            return f;
        }

        public DiskFolder AddFolder(string name)
        {
            DiskFolder folder = new DiskFolder(name, this);
            Folders.Add(folder);

            _size = null;
            return folder;
        }

        public List<DiskFolder> GetAllChildrenFolders()
        {
            return Folders.SelectMany(z => z.GetAllChildrenFolders()).Prepend(this).ToList();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            List<string> input = File.ReadAllLines("input.txt").ToList();
            DiskFolder root = ParseFileLines(input);
            int totalSizeUsed = root.Size;

            List<DiskFolder> flatFiles = root.GetAllChildrenFolders();
            

            List<DiskFolder> largesTotalFilesList = flatFiles.Where(z => z.Size < 100000).ToList();
            int largeFoldersTotal = flatFiles.Select(z => z.Size).Sum();

            Console.WriteLine(largeFoldersTotal);


            List<DiskFolder> flatFilesSorted = flatFiles.OrderBy(z => z.Size).ToList();
            DiskFolder? fileToDelete = flatFilesSorted.FirstOrDefault(z => (totalSizeUsed - z.Size) < (70000000 - 30000000));
            Console.WriteLine(fileToDelete.Name);

        }

        static DiskFolder ParseFileLines(List<string> lines)
        {
            DiskFolder currentFolder = new DiskFolder("/", null);
            string currentCommand = "";

            int currentLineIndex = 0;
            while(currentLineIndex < lines.Count)
            {
                string line = lines[currentLineIndex];
                string[] parts = line.Split(' ');
                if (parts.Length == 0)
                    continue;

                if (parts[0] == "$")
                {
                    currentCommand = parts[1]!;
                    if (parts[1] == "cd")
                    {
                        currentFolder = currentFolder.Navigate(parts[2]);
                    }
                    else if (parts[1] == "ls")
                    {
                        // We dont need to do anything - the currentCommand's been recorded
                    }
                }
                else
                {
                    if(currentCommand == "ls")
                    {
                        if (parts[0] == "dir")
                        {
                            currentFolder.AddFolder(parts[1]);
                        }
                        else
                        {
                            int.TryParse(parts[0], out int size);
                            currentFolder.AddFile(parts[1], size);
                        }
                    }
                }

                currentLineIndex++;
               
            }
            return currentFolder.Navigate("/");
        }
    }
}