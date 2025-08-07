public interface IUpgradeableBuilding
{
    string GetDisplayName();         // bijv. “Glasoven”
    int GetMaxLevel();              // totaal aantal levels
    string GetLevelInfo(int level); // string met info voor dit level (bijv. productie, queue)
}
