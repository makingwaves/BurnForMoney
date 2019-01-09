import React from 'react';
import './Ranking.css';

const Ranking = (props) =>{
  let rank = 0;
  let rankSkip = 1;
  let prevPoints = 0;
   return (
    <div className="Ranking">
      <h4>Rank</h4>
      <ul className="RankingFilterList">
        <li className="RankingFilterListItem active">All</li>
        {props.categories.map((i) =>{
            return(
              <li className="RankingFilterListItem" key={i.category}>{i.category}</li>
            );
        })}
      </ul>
      <h4 className="RankingCategory">{props.rankCategory}</h4>
      <ol className="RankingList">
        {
          props.ranking.map( (i, index)=> {
          if(i.points === prevPoints){
            rankSkip++;
          } else {
            rank = rank + rankSkip;
            rankSkip = 1;
          }
          prevPoints = i.points;

          return(
            <li key={i.athleteId} className={`RankingListItem ${rank === 1 ? 'leader' : ''}`}>
              <div className="RankingListItem-rank">{rank}</div>
              <div className="RankingListItem-avatar">
                <img src={i.profilePictureUrl} alt="" className="RankingListItem-image"/>
              </div>
              <div className="RankingListItem-name">{`${i.athleteFirstName} ${i.athleteLastName}`}</div>
              <div className="RankingListItem-points">{i.points}</div>
            </li>
          );
        })}

      </ol>
    </div>
  )
}

export default Ranking;
