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
                value.Design = Design;

                base.Add(key, value);
                InvokeChanged();
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
                    if (!ContainsKey(value.Name)) Add(value.Name, value);
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
                if (ContainsKey(value.Name))
                {
                    Remove(value.Name);
                    InvokeChanged();
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
                var ls = Values.ToList();

                Clear();
                foreach (var v in ls) Add(target);
            }
            return ret;
        }
        #endregion
        #region Reset
        public void Reset()
        {
            foreach (var vk in Keys)
                this[vk].Design = Design;
        }
        #endregion

        #region InvokeChanged
        protected void InvokeChanged() => Changed?.Invoke(this, null);
        #endregion
        #endregion
    }
}
