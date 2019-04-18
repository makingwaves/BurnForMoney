import React from 'react';
import './Participants.css';

import DashboardHeader from '../../components/DashboardHeader/DashboardHeader.js';
import RankingList from '../../../../components/Ranking/RankingList/RankingList';
import RankingFilter from '../../components/RankingFilter/RankingFilter.js';
import RankingSearch from './components/RankingSearch/RankingSearch.js';


const Participants = (props) =>{

  return (
    <React.Fragment>
      <DashboardHeader header="Participants" />
      <div className="Dashboard-content Participants">
        <RankingSearch
          ranking={props.ranking}
          setRankingInputFilter={props.setRankingInputFilter}
        />
        <RankingFilter
          rankingCategory={props.rankingCategory}
          setRankinkCategory={props.setRankinkCategory}
          categories={props.categories}
        />
        <div className="ParticipantsRanking">
          <h4 className="RankingCategory">{props.rankingCategory}</h4>
          <RankingList
            ranking={props.ranking}
            rankingLoading={props.rankingLoading}
            rankingInputFilter={props.rankingInputFilter}
          />
        </div>
      </div>
    </React.Fragment>
  )
}

export default Participants;
