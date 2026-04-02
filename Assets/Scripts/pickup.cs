using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    public enum pickupType { coin,gem,health}

    public pickupType pt;
    [SerializeField] GameObject PickupEffect;
    [SerializeField] AudioClip coin_pickup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetComponent<Enemy>() != null)
        {
            return;
        }

        if(pt == pickupType.coin)
        {
            if(collision.gameObject.tag == "Player")
            {
                GameManager.instance.IncrementCoinCount();
                
                if(coin_pickup != null)
                    AudioSource.PlayClipAtPoint(coin_pickup, transform.position);
           
                if (PickupEffect != null)
                {
                    Instantiate(PickupEffect, transform.position, Quaternion.identity);
                }

                Destroy(this.gameObject,0.2f);
                
            }
            
        }

        if (pt == pickupType.gem)
        {
            if (collision.gameObject.tag == "Player")
            {
                GameManager.instance.IncrementGemCount();
            
                if (PickupEffect != null)
                {
                    Instantiate(PickupEffect, transform.position, Quaternion.identity);
                }

                Destroy(this.gameObject, 0.2f);

            }

        }
    }
}
