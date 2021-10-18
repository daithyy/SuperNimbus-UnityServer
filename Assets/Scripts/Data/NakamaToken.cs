using System;

[Serializable]
public class NakamaResult
{
    public NakamaToken result;
}

[Serializable]
public class NakamaToken
{
    public string collection;

    public string userId;

    public int createTime;

    public int updateTime;

    public string key;

    public string version;

    public int permissionRead;

    public int permissionWrite;

    public Token value;
}

[Serializable]
public class Token
{
    public string token;
}
