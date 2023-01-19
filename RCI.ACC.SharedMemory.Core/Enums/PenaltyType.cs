namespace RCI.ACC.SharedMemory.Core.Enums;

public enum PenaltyType
{
  None,
  DriveThroughCutting,
  StopAndGo10Cutting,
  StopAndGo20Cutting,
  StopAndGo30Cutting,
  DisqualifiedCutting,
  RemoveBestLaptimeCutting,

  DriveThroughPitSpeeding,
  StopAndGo10PitSpeeding,
  StopAndGo20PitSpeeding,
  StopAndGo30PitSpeeding,
  DisqualifiedPitSpeeding,
  RemoveBestLaptimePitSpeeding,

  DisqualifiedIgnoredMandatoryPit,

  PostRaceTime,
  DisqualifiedTrolling,
  DisqualifiedPit,
  DisqualifiedPitExit,
  DisqualifiedWrongWay,

  DriveThroughIgnoredDriverStint,
  DisqualifiedIgnoredDriverStint,

  DisqualifiedExceededDriverStintLimit,
};