using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthbar : MonoBehaviour {

    /// <summary>
    /// Image of the healthbar fill.
    /// </summary>
    [SerializeField] private Image m_EnemyHealthbar;

    /// <summary>
    /// Changes the enemy healthbar to current health value.
    /// </summary>
    /// <param name="newHealthValue">Enemy current health</param>
    public void ChangeEnemyHealthUI(float newHealthValue)
    {
         m_EnemyHealthbar.fillAmount = newHealthValue;
    }
}