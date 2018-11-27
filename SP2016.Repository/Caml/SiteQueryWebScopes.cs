namespace SP2016.Repository.Enums
{
    public enum SiteQueryWebScope
    {
        //This will search the entire site collection no matter which web you use to execute the query.
        SiteCollection,
        //This will search the web on which you execute the query and recurse through any child webs.
        Recursive,
        //If you leave it blank then it will only search the web on which you execute the query. No child webs will be queried.
        CurrentSiteOnly
    }
}
