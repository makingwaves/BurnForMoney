import React from 'react';
//import './Participants.css';

import DashboardHeader from '../DashboardHeader/DashboardHeader.js';
import RankingList from '../RankingList/RankingList.js';
import RankingFilter from '../RankingFilter/RankingFilter.js';
import RankingSearch from '../RankingSearch/RankingSearch.js';


const Participants = (props) =>{

  return (
    <div>
      <DashboardHeader header="Participants" />
      <div className="Dashboard-content">
        <RankingSearch
          ranking={props.ranking}
          setRankingInputFilter={props.setRankingInputFilter}
        />
        <RankingFilter
          rankingCategory={props.rankingCategory}
          setRankinkCategory={props.setRankinkCategory}
          categories={props.categories}
        />
        <h4 className="RankingCategory">{props.rankingCategory}</h4>
        <RankingList
          ranking={props.ranking}
          rankingInputFilter={props.rankingInputFilter}
        />
      </div>

    </div>
  )
}

export default Participants;
