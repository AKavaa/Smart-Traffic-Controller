public interface IPedestrianSignalManager
{
    string GetStatus();
    bool SetWalk(bool on);
    bool SetAudible(bool on);
    bool SetWait(bool on);

}