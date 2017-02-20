using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioPlayerV2
{
    public class AudioFile
    {

        public AudioFile(string name, string filepath)
        {
            Name = name;
            FilePath = filepath;
        }

        public string Name { get; set; }

        public string FilePath { get; set; }

    }
}
