namespace fsm.triggers
{
    public class OnTutorialDataChanged : ITrigger
    {
        public int id;
        public OnTutorialDataChanged(int id)
        {
            this.id = id;
        }
    }
}
