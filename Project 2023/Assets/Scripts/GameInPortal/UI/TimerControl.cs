using UnityEngine ;

public class TimerControl : MonoBehaviour {

   [SerializeField] Timer timer ;
   public PortalPlaceSet portalPlaceSet;
   public CountStone countStone;
   public void CountStart() {
      if(portalPlaceSet.state == 3){
         countStone.stonenum = 0;
         countStone.stoneNum.text = countStone.stonenum.ToString();
      }
      timer
      .SetDuration (120)
      .OnEnd (() => portalPlaceSet.Change())
      .Begin () ;
   }

}
