using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

// Marks this job for compilation with the Burst compiler for high performance
[BurstCompile]
public struct FindNearestJob : IJobParallelFor
{
    // Read-only arrays for target and seeker positions to prevent modification during job execution
    [ReadOnly] public NativeArray<float3> TargetPositions;
    [ReadOnly] public NativeArray<float3> SeekerPositions;

    // An array to store the position of the nearest target for each seeker
    public NativeArray<float3> NearestTargetPositions;

    // The method executed for each seeker to find its nearest target
    public void Execute(int index)
    {
        // Retrieve the position of the current seeker
        float3 seekerPos = SeekerPositions[index];

        // Perform a binary search on the sorted target positions to find the closest target by X coordinate
        int startIdx = TargetPositions.BinarySearch(seekerPos, new AxisXComparer { });

        // BinarySearch returns the complement of the index where the value should be inserted if not found
        // Check if startIdx is negative, indicating the target was not found, and adjust the index accordingly
        if (startIdx < 0) startIdx = ~startIdx; // Flip the bits back to find the intended index
        // Ensure the corrected index does not exceed the bounds of the target array
        if (startIdx >= TargetPositions.Length) startIdx = TargetPositions.Length - 1;

        // Initialize with the target position found by binary search as the closest
        float3 nearestTargetPos = TargetPositions[startIdx];
        // Calculate the square of the distance to this initial nearest target
        float nearestDistSq = math.distancesq(seekerPos, nearestTargetPos);

        // Search for a closer target above the initial index
        Search(seekerPos, startIdx + 1, TargetPositions.Length, +1, ref nearestTargetPos, ref nearestDistSq);
        // Search for a closer target below the initial index
        Search(seekerPos, startIdx - 1, -1, -1, ref nearestTargetPos, ref nearestDistSq);

        // Update the nearest target position for the current seeker
        NearestTargetPositions[index] = nearestTargetPos;
    }

    // Searches for a closer target in a specified direction (upward or downward)
    void Search(float3 seekerPos, int startIdx, int endIdx, int step,
                ref float3 nearestTargetPos, ref float nearestDistSq)
    {
        // Iterate through the target positions in the specified direction
        for (int i = startIdx; i != endIdx; i += step)
        {
            // Current target position being examined
            float3 targetPos = TargetPositions[i];
            // Calculate the x-axis distance between the seeker and the current target
            float xdiff = seekerPos.x - targetPos.x;

            // If the square of the x-axis distance is greater than the nearest distance squared, stop searching
            if ((xdiff * xdiff) > nearestDistSq) break;

            // Calculate the square of the two-dimensional distance to the current target
            float distSq = math.distancesq(targetPos, seekerPos);

            // If this distance is less than the current nearest distance squared, update the nearest target
            if (distSq < nearestDistSq)
            {
                nearestDistSq = distSq;
                nearestTargetPos = targetPos;
            }
        }
    }
}

// Custom comparer for sorting or searching based on the x-coordinate of float3 positions
public struct AxisXComparer : IComparer<float3>
{
    // Compares two float3 positions by their x-coordinate
    public int Compare(float3 a, float3 b)
    {
        return a.x.CompareTo(b.x);
    }
}
