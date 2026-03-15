namespace Arunoki.Flow.Utilities
{
  public static class IndexExtensions
  {
    public static int Wrap (this int index, int count)
    {
      if (count <= 0)
        return 0;

      return ((index % count) + count) % count;
    }

    public static int Next (this int index, int count)
    {
      if (count <= 0)
        return 0;

      return (index + 1) % count;
    }

    public static int Previous (this int index, int count)
    {
      if (count <= 0)
        return 0;

      return (index - 1 + count) % count;
    }
  }
}