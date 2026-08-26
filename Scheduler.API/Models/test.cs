namespace Scheduler.API.Models
{
    public class test
    {
        public virtual void test1()
        { }

    }
    public class testB:test
    {
        public override void test1()
        {
            testAbstract testAbstract = new testBB();
           
        }

    }

    public abstract class testAbstract
    {
        public int MyProperty { get; set; }
        public abstract void test1();   

        public void test2()
        { }

        private void test2(int x)
        { }

        
    }
    public class testBB : testAbstract
    {
        public override void test1()
        { }

    }
}
