//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//// Source from: http://blogs.msdn.com/b/pfxteam/archive/2012/02/12/10266983.aspx
//namespace Breeze.Sharp.Core {

//  public class AsyncSemaphore {
    
//    public AsyncSemaphore(int initialCount) {
//      if (initialCount < 0) throw new ArgumentOutOfRangeException("initialCount");
//      m_currentCount = initialCount;
//    }

//    public Task WaitAsync() {

//#if !__WASM__
//            lock (m_waiters)
//#endif
//            {
//        if (m_currentCount > 0) {
//          --m_currentCount;
//          return s_completed;
//        } else {
//          var waiter = new TaskCompletionSource<bool>();
//          m_waiters.Enqueue(waiter);
//          return waiter.Task;
//        }
//      }
//    }

//    public void Release() {
//      TaskCompletionSource<bool> toRelease = null;
//#if !__WASM__
////            lock (m_waiters)
//#endif
//            {
//                if (m_waiters.Count > 0)
//                {
//                    m_waiters.TryDequeue(out toRelease);
//                }
//                else
//                {
//                    ++m_currentCount;
//                }
//      }
//      if (toRelease != null)
//        toRelease.SetResult(true);
//    }

//    private readonly static Task s_completed = Task.FromResult(true); 
//    private readonly ConcurrentQueue<TaskCompletionSource<bool>> m_waiters = new ConcurrentQueue<TaskCompletionSource<bool>>(); 
//    private int m_currentCount; 


//  }
//}
