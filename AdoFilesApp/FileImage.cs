using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdoFilesApp
{
    internal class FileImage
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public string Description { set; get; }
        public byte[] Data { set; get; }

        public FileImage(int id, string name, string description, byte[] data)
        {
            Id = id;
            Name = name;
            Description = description;
            Data = data;
        }
    }
}
