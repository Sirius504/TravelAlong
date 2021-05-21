using System;

public class MovementData
{
    public readonly float distance;
    public readonly float initialSpeed;
    public readonly float targetSpeed;
    public readonly float accelerationRate;
    public readonly float decelerationRate;

    private float accelerationEndTime;
    private float decelerationStartTime;
    private float stopTime;

    public MovementData(float distance, float initialSpeed, float targetSpeed, float accelerationRate, float decelerationRate)
    {
        this.distance = Math.Max(distance, 0f);
        this.initialSpeed = initialSpeed;
        this.targetSpeed = targetSpeed;
        this.accelerationRate = accelerationRate < 0 ? Math.Abs(accelerationRate) : accelerationRate;
        this.decelerationRate = decelerationRate < 0 ? Math.Abs(decelerationRate) : decelerationRate;

        accelerationEndTime = (targetSpeed - initialSpeed) / accelerationRate;
        decelerationStartTime = 0f;
        stopTime = 0f;
        decelerationStartTime = CalculateDecelerationStartTime();
        stopTime = decelerationStartTime + GetSpeedIgnoreDeceleration(decelerationStartTime) / decelerationRate;
    }

    public float GetDistance(float time)
    {
        if (time >= stopTime)
            return distance;

        if (time <= decelerationStartTime)
        {
            return GetDistanceIgnoreDeceleration(time);
        }
        else
        {
            float decelerationDuration = time - decelerationStartTime;
            return GetDistanceIgnoreDeceleration(decelerationStartTime)
                + GetSpeedIgnoreDeceleration(decelerationStartTime) * decelerationDuration
                - decelerationRate * decelerationDuration * decelerationDuration / 2;
        }
    }

    public float GetSpeed(float time)
    {
        if (distance <= 0f || time >= stopTime)
            return 0f;

        if (time <= 0)
            return initialSpeed;

        if (time <= decelerationStartTime)
            return GetSpeedIgnoreDeceleration(time);
        else
        {
            float decelerationDuration = time - decelerationStartTime;
            return GetSpeedIgnoreDeceleration(decelerationStartTime) - decelerationRate * decelerationDuration;
        }
    }
    private float CalculateDecelerationStartTime()
    {
        float result = Helper.SolveBisection(DecelerationStartTimeEquation, 0f, distance * 10f, true);
        float check = DecelerationStartTimeEquation(result);
        return result;
    }

    private float DecelerationStartTimeEquation(float time)
    {
        float a = GetDistanceIgnoreDeceleration(time);
        float b = GetDecelerationDistance(GetSpeedIgnoreDeceleration(time));        
        float result = GetDistanceIgnoreDeceleration(time) + GetDecelerationDistance(GetSpeedIgnoreDeceleration(time)) - distance;
        return result;
    }

    private float GetDecelerationDistance(float speed)
    {
        float time = speed / decelerationRate;
        return speed * time - (decelerationRate * time * time) / 2;
    }

    private float GetSpeedIgnoreDeceleration(float time)
    {
        if (time <= accelerationEndTime)
            return initialSpeed + accelerationRate * time;
        else
            return targetSpeed;
    }

    private float GetDistanceIgnoreDeceleration(float time)
    {
        if (time <= accelerationEndTime)
            return initialSpeed * time + accelerationRate * time * time / 2;
        else
            return GetDistanceIgnoreDeceleration(accelerationEndTime) + targetSpeed * (time - accelerationEndTime);
    }
}