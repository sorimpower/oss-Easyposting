using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace csharptest
{
    class fileController
    {
        // 해당 경로에 폴더가 없다면 디렉토리 생성
        public void CreateDirectory(string path)
        {
            DirectoryInfo di = new DirectoryInfo(path);
            if (di.Exists == false)
            {
                di.Create();
            }
        }

        // string 저장
        public void SaveContents(string path, string name, string data)
        {
            CreateDirectory(path);
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(path + name))
            {
                file.WriteLine(data);
            }
        }

    }
}
