namespace Market
{
    public enum Action
    {
        Buy = 1,
        Hold = 0,
        Sell = -1
    }

    public enum Term
    {
        Short = 1,
        Intermediate = 2,
        Long = 3,
        Unlimited = 99
    }
}