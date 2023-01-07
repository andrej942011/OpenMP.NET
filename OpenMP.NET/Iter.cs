﻿using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenMP
{
    internal static class Iter
    {
        internal static void StaticLoop(object thread_id)
        {
            int tid = (int)thread_id;
            Thr thr = Init.ws.threads[tid];

            thr.curr_iter = (int)(Init.ws.start + tid * Init.ws.chunk_size);
            int end = Init.ws.end;

            while (thr.curr_iter < end)
                StaticNext(thr, tid, Init.ws.chunk_size);

            Interlocked.Add(ref Init.ws.threads_complete, 1);
        }

        private static void StaticNext(Thr thr, int thread_id, uint chunk_size)
        {
            int start = thr.curr_iter;
            int end = (int)Math.Min(thr.curr_iter + chunk_size, Init.ws.end);
            Action<int> omp_fn = Init.ws.omp_fn;

            ref int i = ref thr.working_iter;

            for (i = start; i < end; i++)
            {
                omp_fn(i);
                if (Init.ws.brack_loop)
                {
                    break;
                }
            }

            Init.ws.brack_loop = false;

            thr.curr_iter += (int)(Init.ws.num_threads * chunk_size);
        }

        internal static void DynamicLoop(object thread_id)
        {
            int tid = (int)thread_id;
            Thr thr = Init.ws.threads[tid];
            int end = Init.ws.end;

            while (Init.ws.start < end)
            {
                DynamicNext(thr);
            }

            Interlocked.Add(ref Init.ws.threads_complete, 1);
        }

        private static void DynamicNext(Thr thr)
        {
            int chunk_start;

            lock (Init.ws.ws_lock)
            {
                chunk_start = Init.ws.start;
                Init.ws.start += (int)Init.ws.chunk_size;
            }

            int chunk_end = (int)Math.Min(chunk_start + Init.ws.chunk_size, Init.ws.end);
            Action<int> omp_fn = Init.ws.omp_fn;

            ref int i = ref thr.working_iter;

            for (i = chunk_start; i < chunk_end; i++)
            {
                omp_fn(i);
            }
        }

        internal static void GuidedLoop(object thread_id)
        {
            int tid = (int)thread_id;
            Thr thr = Init.ws.threads[tid];
            int end = Init.ws.end;

            while (Init.ws.start < end)
            {
                GuidedNext(thr);
            }

            Interlocked.Add(ref Init.ws.threads_complete, 1);
        }

        private static void GuidedNext(Thr thr)
        {
            int chunk_start, chunk_size;

            lock (Init.ws.ws_lock)
            {
                chunk_start = Init.ws.start;
                chunk_size = (int)Math.Max(Init.ws.chunk_size, (Init.ws.end - chunk_start) / Init.ws.num_threads);
                Init.ws.start += chunk_size;
            }

            int chunk_end = (int)Math.Min(chunk_start + chunk_size, Init.ws.end);
            Action<int> omp_fn = Init.ws.omp_fn;

            ref int i = ref thr.working_iter;

            for (i = chunk_start; i < chunk_end; i++)
            {
                omp_fn(i);
            }
        }
    }
}
