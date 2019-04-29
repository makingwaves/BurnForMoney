import React from 'react';
import './RankingFilter.css';

const RankingFilter = (props) =>{
let setRankingCategory = props.setRankinkCategory;
  return (
      <ul className="RankingFilter">
        <li className={`RankingFilterItem ${props.rankingCategory === 'All' && "active"}`} onClick={() => setRankingCategory('All')}>All</li>
        {props.categories.map((i) =>{
            return(
              <li className={`RankingFilterItem ${props.rankingCategory === i.category && "active"}`} key={i.category} onClick={() => {setRankingCategory(i.category);} }>{i.category}</li>
            );
        })}
      </ul>
  )
}

export default RankingFilter;
