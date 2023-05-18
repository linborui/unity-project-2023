using UnityEngine ;

public class TimerControl : MonoBehaviour {

   [SerializeField] Timer timer ;
   public PortalPlaceSet portalPlaceSet;
   public StoneManagement countStone;
   public void CountStart() {
      if(portalPlaceSet.state == 3){
         countStone.stoneNumSet(0);
      }
      timer
      .SetDuration (120)
      .OnEnd (() => portalPlaceSet.Change())
      .Begin () ;
   }

}
