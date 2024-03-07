using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
namespace Ping_CMD
{
    class Program
    {
        static void check(ref string input)
        {
            Ping p;
            int success = 0; // so byte thanh cong
            int count = 1; // so lan ping
            int numPacket = 4; // so luong packet muon ping
            List<int> roundtrip = new List<int>(); // thoi gian ping
            int timeout = 4000; // 4s het thoi gian => request timeout
            int size = 32; // kich thuoc goi tin
            Console.OutputEncoding = Encoding.UTF8; // tieng viet co dau
            string[] arr = input.Trim().Split(' ');
            if (arr[0].ToLower().StartsWith("ping"))
            {
                Console.WriteLine(input);
                Console.WriteLine("================Press esc to exit ping!!================ ");
                Console.WriteLine("Pinging " + arr[1] + " with "+ size +" bytes of data:");
                while (count <= numPacket)
                {
                    if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape) // bat su kien phim esc, khong dung man hinh
                    {
                        // stop ping lien tuc
                        break;
                    }
                    try
                    {
                        if (arr[2] == "/t")
                        {
                            numPacket++;
                        }
                    }
                    catch
                    {

                    }
                    byte[] buffer = new byte[size]; // kich thuoc goi tin la kich thuoc buffer
                    Thread.Sleep(500);
                    p = new Ping(); // tao lop ho tro Ping
                    PingReply pr;
                    try
                    {
                        pr = p.Send(arr[1], timeout, buffer); // gui ip thoi gian timeout va kich thuoc goi tin
                        if (pr.Status == IPStatus.DestinationHostUnreachable) // neu khong tim thay dich den
                        {
                            string thongbao = "Destination Host Unreachable!";
                            Console.WriteLine(arr[1]);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine(thongbao);
                            Console.ResetColor();
                            success++;
                        }
                        if (pr.Status == IPStatus.TimedOut) // nếu hết thời gian ping
                        {
                            string thongbao = "Oops! Request Timeout!";
                            Console.WriteLine(arr[1]);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(thongbao);
                            Console.ResetColor();
                        }
                        if (pr.Status == IPStatus.Success) // neu ping thành công
                        {
                            Thread.Sleep(500);
                            Console.WriteLine($"Reply from {pr.Address.ToString()}: bytes={ pr.Buffer.Length.ToString()} time<{ pr.RoundtripTime.ToString()}ms TTL={pr.Options.Ttl.ToString()}");
                            success++;
                            if (pr.RoundtripTime != 0) // nếu thời gian ping khác 0 thì thêm vào list roundTrip
                                roundtrip.Add(Convert.ToInt32(pr.RoundtripTime));
                        }
                        if (pr.Status == IPStatus.Unknown)
                        {
                            string thongbao = "Chưa biết nguyên nhân !";
                            Console.WriteLine(arr[1]);
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.WriteLine(thongbao);
                            Console.ResetColor();
                        }
                    }
                    catch
                    {
                        Console.WriteLine("IP hoặc Host không tồn tại ! Mời nhập lại!");
                        return;
                    }
                    count++; // tăng số lần ping 
                }
                count = count - 1; // nếu người dùng thoát vòng lặp phải trừ 1 do ở trên cộng 1
                int lost = count - success;
                double phantramlost = Math.Round((((double)lost / (double)numPacket) * 100),
                0);
                Console.WriteLine($"Ping statistics for {arr[1]}" );
                Console.WriteLine(" Packets: Send = " + (count).ToString() + ", Received = " +
                success.ToString() + ", Lost = " + lost.ToString() + " ( " + phantramlost.ToString() + "% loss)");
                if (roundtrip.Count != 0)
                {
                    Console.WriteLine("Approximate round trip times in mili-second: ");
                    int max = roundtrip.Max();
                    int min = roundtrip.Min();
                    int avegare = (min + max) / 2;
                    Console.WriteLine("Minimun = " + min.ToString() + "ms, Maximum = " +
                    max.ToString() + "ms, Average = " + avegare.ToString() + "ms");
                }
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Nhap sai lenh");
            }

        }
        static void xulychuoiping(ref string input)
        {
            string[] result; // tao mang
            input = input.ToLower(); // in thuong chuoi nhap vao
            string output = ""; // tao chuoi ket qua
            // Xóa khoảng trắng ở giữa
            foreach (string word in input.Split(' ', (char)StringSplitOptions.RemoveEmptyEntries))
            {
                output += word.Trim().ToLower();
            }
            bool check = false;

            if (output.Contains("/t"))
            {
                int step = 0;
                int ind;
                int temp;
                while (true)
                {
                    ind = output.IndexOf('t', step + 1);
                    if (output[ind - 1] == '/')
                    {
                        break;
                    }
                    step = ind;
                }
                temp = ind - 1;
                output = output.Replace("/", "");
                output = output.Insert(temp, "/");
                Console.WriteLine("chao");
                check = true;
            }
            else // khong nhap "/?"
            {
                string ping = output.Substring(0, 4);
                string ip = output.Substring(4);
                input = ping + " " + ip;
                check = false;
            }
            if (check == true)
            {
                int index = output.IndexOf('/');
                string i2 = output.Substring(index, 2);
                result = output.Split(new[] { i2 }, StringSplitOptions.None); // bo /t o giua
                string com = String.Join("", result) + " " + i2;
                com = com.Insert(4, " ");
                input = com;
            }
        }
        static void Main(string[] args)
        {
            while (true)
            {
                string input = Console.ReadLine();
                xulychuoiping(ref input);
                check(ref input);
            }
        }
    }
}
