namespace FileCheck.ViewModels.Base
{
    public static class VMLocator
    {
        static MainVM main;
        public static MainVM Main =>main??=new MainVM();
    }
}
