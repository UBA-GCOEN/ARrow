using UnityEngine;

namespace Gpm.Manager.Ui
{
    internal abstract class UiView
    {
        protected readonly GpmManagerWindow window;

        protected UiView(GpmManagerWindow window)
        {
            this.window = window;
        }

        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }

        public virtual void OnDestroy()
        {
        }

        public abstract void OnGUI(Rect rect);

        public virtual void Update()
        {
        }

        public virtual void Clear()
        {
        }
    }
}