import React from 'react';
import './RankingSearch.css';

const RankingSearch = (props) => {
  let setRankingInputFilter = props.setRankingInputFilter;

  return (
    <div className="RankingSearch">
      <input type="text" placeholder="Search by name" value={props.rankingInputFilter} onChange={(e) => {setRankingInputFilter(e.target.value);} }/>
    </div>
  )
}

export default RankingSearch;
