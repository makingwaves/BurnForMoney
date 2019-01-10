import React from 'react';
//import './Participants.css';

import DashboardHeader from '../DashboardHeader/DashboardHeader.js';
import Ranking from '../Ranking/Ranking.js';

const Participants = (props) =>{
  return (
    <div>
      <DashboardHeader header="Participants" />
      <div className="Dashboard-content">
        <Ranking
          ranking={props.ranking}
          rankingCategory={props.rankingCategory}
          setRankinkCategory={props.setRankinkCategory}
          categories={props.categories}
          rankCategory={props.rankCategory}
        />
      </div>
    </div>
  )
}

export default Participants;
