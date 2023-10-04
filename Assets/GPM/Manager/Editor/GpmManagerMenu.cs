using Gpm.Manager.Ui;
using UnityEditor;

namespace Gpm.Manager
{
    public static class GpmManagerMenu
    {
#if GPM_DEBUG
        private const string MENU_OPEN_MANAGER = "Tools/GPM/Manager #t";
#else
        private const string MENU_OPEN_MANAGER = "Tools/GPM/Manager";
#endif

        [MenuItem(MENU_OPEN_MANAGER)]
        private static void OpenManager()
        {
            GpmManagerWindow.OpenWindow();
        }
    }
}