using Devinno.Skia.Design;
using Devinno.Skia.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Collections
{
    public class DvPageCollection : Dictionary<string, DvPage>
    {
        #region Member Variable
        private DvDesign Design;
        #endregion

        #region Event
        public event EventHandler Changed;
        #endregion

        #region Constructor
        public DvPageCollection(DvDesign Design)
        {
            this.Design = Design;
        }
        #endregion

        #region Method
        #region Add
        private new void Add(string key, DvPage value)
        {
            if (value != null)
            {
                /*
                if (value.Name == null)
                {
                    var name = value.GetType().Name;
                    name = char.ToLower(name[0]) + name.Substring(1);
                    var ls = Values.Where(x => x.Name.StartsWith(name)).Select(x => Convert.ToInt32(name.Substring(name.Length))).ToList();
                    int n = ls.Count > 0 ? ls.Max() + 1 : 1;
                    value.Name = name + n;
                }
                */
                value.Design = Design;
                base.Add(key, value);
                Changed?.Invoke(this, null);
            }
            else throw new Exception("페이지가 null 일 수 없습니다.");
        }

        public void Add(DvPage value)
        {
            if (value != null)
            {
                value.Design = Design;
                if (value.Name != null)
                {
                    if (!this.ContainsKey(value.Name))
                    {
                        this.Add(value.Name, value);
                        Changed?.Invoke(this, null);
                    }
                    else throw new Exception("동일한 이름의 페이지가 존재합니다.");
                }
                else throw new Exception("페이지 이름은 null 일 수 없습니다.");
            }
            else throw new Exception("페이지가 null 일 수 없습니다.");
        }
        #endregion
        #region Remove
        public void Remove(DvPage value)
        {
            if (value != null)
            {
                if (this.ContainsKey(value.Name))
                {
                    this.Remove(value.Name);
                    Changed?.Invoke(this, null);
                }
            }
        }
        #endregion
        #region Rename
        public bool Rename(DvPage target, string NewName)
        {
            var ret = false;
            if (!ContainsKey(NewName))
            {
                target.Name = NewName;
                var ls = this.Values.ToList();

                this.Clear();
                foreach (var v in ls) Add(target);
            }
            return ret;
        }
        #endregion
        #endregion
    }
}
