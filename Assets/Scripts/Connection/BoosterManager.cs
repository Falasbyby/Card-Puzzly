using Messages;

public class BoosterManager : Singleton<BoosterManager>
{
    private AppliedBoostsMessage appliedBoostsMessage;
    public bool rocketActive = false;
    public bool boostStar = false;
    
    public void Initialize(AppliedBoostsMessage message)
    {
        appliedBoostsMessage = message;

     //   CustomDebug.Log($"Boosts initialized: {message.boosts.Length} boosts received");
        ApplyBoosts();
       
    }

    private void ApplyBoosts()
    {
            /* foreach (var boost in appliedBoostsMessage.boosts)
            {
                CustomDebug.Log($"Boost Type: {boost.type}, Enabled: {boost.enabled}");

                switch (boost.type)
                {
                    case "ROCKET":
                        ActivateRoсket(boost.enabled);
                        break;

                    case "NUT_BOOST":
                        ActivateStarBoost(boost.enabled);
                        break;

                    case "MAGNET":
                        ActivateMagnet(boost.enabled);
                        break;

                    default:
                        CustomDebug.LogWarning($"Unknown boost type: {boost.type}");
                        break;
                }
            } */
    }

    private void ActivateRoсket(bool active)
    {
        if (active)
        {
            CustomDebug.Log("ExtraLife booster activated!");
            rocketActive = true;
            //LifeManager.Instance.BoosterActive();
        }
        else
        {
            CustomDebug.Log("ExtraLife booster deactivated.");
        }
    }

    private void ActivateStarBoost(bool active)
    {
        
        CustomDebug.Log($"PointBoost set to: {active}");
        boostStar = active;
        //  GameController.Instance.SetCoinMultiplier(active);
    }

    private void ActivateMagnet(bool active)
    {
        CustomDebug.Log($"StarBoost set to: {active}");
       // Magnet.Instance.ActiveMagnet(active);
       // CoinManager.Instance.SetBoosterMultiplier(active);
    }
}