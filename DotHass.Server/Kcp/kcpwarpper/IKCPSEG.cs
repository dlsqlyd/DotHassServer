using System;
using System.Runtime.InteropServices;

//using System.Threading.Tasks;

namespace kcpwarpper
{
    //https://docs.microsoft.com/zh-cn/dotnet/standard/class-library-overview
    public unsafe struct IQUEUEHEAD
    {
        public IQUEUEHEAD* next;
        public IQUEUEHEAD* prev;
    };

    //=====================================================================
    // SEGMENT
    //=====================================================================
    public unsafe struct IKCPSEG
    {
        public IQUEUEHEAD node;
        public uint conv;
        public uint cmd;
        public uint frg;
        public uint wnd;
        public uint ts;
        public uint sn;
        public uint una;
        public uint len;
        public uint resendts;
        public uint rto;
        public uint fastack;
        public uint xmit;
        fixed sbyte data[1];    
    };

    //---------------------------------------------------------------------
    // IKCPCB
    //---------------------------------------------------------------------
    public unsafe struct IKCPCB
    {
        public uint conv, mtu, mss, state;
        public uint snd_una, snd_nxt, rcv_nxt;
        public uint ts_recent, ts_lastack, ssthresh;
        public int rx_rttval, rx_srtt, rx_rto, rx_minrto;
        public uint snd_wnd, rcv_wnd, rmt_wnd, cwnd, probe;
        public uint current, interval, ts_flush, xmit;
        public uint nrcv_buf, nsnd_buf;
        public uint nrcv_que, nsnd_que;
        public uint nodelay, updated;
        public uint ts_probe, probe_wait;
        public uint dead_link, incr;
        public IQUEUEHEAD snd_queue;
        public IQUEUEHEAD rcv_queue;
        public IQUEUEHEAD snd_buf;
        public IQUEUEHEAD rcv_buf;
        public uint* acklist;
        public uint ackcount;
        public uint ackblock;
        public void* user;
        public char* buffer;
        public int fastresend;
        public int nocwnd, stream;
        public int logmask;

        //int (* output) (const char* buf, int len, struct IKCPCB *kcp, void* user);
        //void (* writelog) (const char* log, struct IKCPCB *kcp, void* user);
        public IntPtr output;

        public IntPtr writelog;
    };

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public unsafe delegate int d_output(sbyte* buf, int len, IKCPCB* kcp, void* user);

    [UnmanagedFunctionPointerAttribute(CallingConvention.Cdecl)]
    public unsafe delegate void d_writelog(sbyte* log, IKCPCB* kcp, void* user);
}