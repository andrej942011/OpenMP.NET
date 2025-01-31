# OpenMP.NET
A library for writing OpenMP-style parallel code in .NET.
Inspired by the fork-join paradigm of OpenMP, and attempts to replicate the OpenMP programming style as faithfully as possible.

## Supported Constructs

### Parallel
Given the OpenMP:
```c
#pragma omp parallel
{
    work();
}
```
OpenMP.NET provides:
```cs
OpenMP.Parallel.ParallelRegion(() => {
    work();
});
```
This function supports the `num_threads` optional parameter, which sets the number of threads to spawn.
The default value is the number of logical threads on the system.

### For
Given the OpenMP:
```c
#pragma omp for
for (int i = a, i < b; i++)
{
    work(i);
}
```
OpenMP.NET provides:
```cs
OpenMP.Parallel.For(a, b, i => {
    work(i);
});
```
This function supports the `schedule` optional parameter, which sets the parallel scheduler to use.
Permissible values are `OpenMP.Parallel.Schedule.Static`, `OpenMP.Parallel.Schedule.Dynamic`, and `OpenMP.Parallel.Schedule.Guided`.
The default value is `OpenMP.Parallel.Schedule.Static`.

This function supports the `chunk_size` optional parameter, which sets the chunk size for the scheduler to use.
The default value is dependent on the scheduler and is not documented, as it may change from version to version.

The behavior of `OpenMP.Parallel.For` is undefined if not used within a `ParallelRegion`.

### Parallel For
Given the OpenMP:
```c
#pragma omp parallel for
for (int i = a, i < b; i++)
{
    work(i);
}
```
OpenMP.NET provides:
```cs
OpenMP.Parallel.ParallelFor(a, b, i => {
    work(i);
});
```
This function supports all of the optional parameters of `ParallelRegion` and `For`, and is merely a wrapper around those two functions for conciseness.

### Critical
Given the OpenMP:
```c
#pragma omp critical
{
    work();
}
```
OpenMP.NET provides:
```cs
OpenMP.Parallel.Critical(id, () => {
    work();
});
```
This function requires an `id` parameter, which is used as a unique identifier for a particular critical region.
If multiple critical regions are present in the code, they should each have a unique `id`.
The `id` should likely be a `const int` or an integer literal.

### Barrier
Given the OpenMP:
```c
#pragma omp barrier
```
OpenMP.NET provides:
```cs
OpenMP.Parallel.Barrier();
```

### Master
Given the OpenMP:
```c
#pragma omp master
{
    work();
}
```
OpenMP.NET provides:
```cs
OpenMP.Parallel.Master(() => {
    work();
});
```
`Master`'s behavior is left undefined if used inside of a `For`.

### Single
Given the OpenMP:
```c
#pragma omp single
{
    work();
}
```
OpenMP.NET provides:
```cs
OpenMP.Parallel.Single(id, () => {
    work();
});
```
The `id` parameter provided should follow the same guidelines as specified in `Critical`.

The behavior of a `single` region is as follows: the first thread in a team to reach a given `Single` region will "own" the `Single` for the duration of the `ParallelRegion`.
On all subsequent encounters, only that first thread will execute the region. All other threads will ignore it.

`Single`'s behavior is left undefined if used inside of a `For`.

### Ordered
Given the OpenMP:
```c
#pragma omp ordered
{
    work();
}
```
OpenMP.NET provides:
```cs
OpenMP.Parallel.Ordered(id, () => {
    work();
});
```
The `id` parameter provided should follow the same guidelines as specified in `Critical`.

`Ordered`'s behavior is left undefined if used outside of a `For`.

## Atomics

OpenMP atomics are implemented as follows:
```c
#pragma omp atomic
a op b;
```
where `op` is some supported operator.

OpenMP.NET supports a subset of this for the `int`, `uint`, `long`, and `ulong` types.
The only implemented atomic operations are `a += b`, `a &= b`, `a |= b`, `++a`, and `--a`.
`a -= b` is implemented, but for signed types only, due to restrictions interfacting with C#'s `Interlocked` class.

The following table documents the supported atomics:

| Operation | OpenMP.NET function           |
------------|--------------------------------
| `a += b`  | `OpenMP.Atomic.Add(ref a, b)` |
| `a -= b`  | `OpenMP.Atomic.Sub(ref a, b)` |
| `a &= b`  | `OpenMP.Atomic.And(ref a, b)` |
| `a \|= b` | `OpenMP.Atomic.Or(ref a, b)`  |
| `++a`     | `OpenMP.Atomic.Inc(ref a)`    |
| `--a`     | `OpenMP.Atomic.Dec(ref a)`    |

For atomic operations like compare-exchange, we recommend interfacting directly with `System.Threading.Interlocked`.
For non-supported atomic operations or types, we recommend using `OpenMP.Parallel.Critical`.
This is more of a limitation of the underlying hardware than anything.

## Supported Functions

OpenMP provides an analog of the following functions:

| <omp.h> function      | OpenMP.NET function             |
------------------------|----------------------------------
| omp_get_num_procs()   | OpenMP.Parallel.GetNumProcs()   |
| omp_get_num_threads() | OpenMP.Parallel.GetNumThreads() |
| omp_get_thread_num()  | OpenMP.Parallel.GetThreadNum()  |
