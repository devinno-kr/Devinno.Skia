using Devinno.Skia.Containers;
using Devinno.Skia.Design;
using Devinno.Skia.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Collections
{
    public class DvSubPageCollection : Dictionary<string, DvSubPage>
    {
        #region Member Variable
        private DvContainer container;
        #endregion

        #region Event
        public event EventHandler Changed;
        #endregion

        #region Constructor
        public DvSubPageCollection(DvContainer container)
        {
            this.container = container;
        }
        #endregion

        #region Method
        #region Add
        private new void Add(string key, DvSubPage value)
        {
            if (value != null)
            {
                value.ParentContainer = container;

                base.Add(key, value);
                Changed?.Invoke(this, null);
            }
            else throw new Exception("페이지가 null 일 수 없습니다.");
        }

        public void Add(DvSubPage value)
        {
            if (value != null)
            {
                value.ParentContainer = container;

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
        public void Remove(DvSubPage value)
        {
            if (value != null)
            {
                if (this.ContainsKey(value.Name))
                {
                    this.Remove(value.Name);
                    Changed?.Invoke(this, null);
                }            }
        }
        #endregion
        #region Rename
        public bool Rename(DvSubPage target, string NewName)
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
