namespace LittleDemo
{
    internal class QuickSortExample
    {
        // 快速排序的主方法
        public static void QuickSort(int[] array, int left, int right)
        {
            if (left < right)
            {
                // 获取分区索引
                int pivotIndex = Partition(array, left, right);

                // 递归对左边部分排序
                QuickSort(array, left, pivotIndex - 1);

                // 递归对右边部分排序
                QuickSort(array, pivotIndex + 1, right);
            }
        }

        // 分区方法
        private static int Partition(int[] array, int left, int right)
        {
            // 选择最右边的元素作为基准元素
            int pivot = array[right];

            // i 表示小于 pivot 的元素的分区索引
            int i = left - 1;

            for (int j = left; j < right; j++)
            {
                // 如果当前元素小于或等于 pivot，将其放到左侧
                if (array[j] <= pivot)
                {
                    i++;
                    Swap(array, i, j);  // 交换元素
                }
            }

            // 将基准元素放到正确的位置
            Swap(array, i + 1, right);
            return i + 1; // 返回分区索引
        }

        // 交换元素
        private static void Swap(int[] array, int a, int b)
        {
            int temp = array[a];
            array[a] = array[b];
            array[b] = temp;
        }

        //// 主程序
        //static void Main(string[] args)
        //{
        //    int[] array = { 10, 80, 30, 90, 40, 50, 70 };
        //    Console.WriteLine("原始数组: " + string.Join(", ", array));

        //    // 调用快速排序
        //    QuickSort(array, 0, array.Length - 1);

        //    // 输出排序后的数组
        //    Console.WriteLine("排序后的数组: " + string.Join(", ", array));
        //}
    }
}
