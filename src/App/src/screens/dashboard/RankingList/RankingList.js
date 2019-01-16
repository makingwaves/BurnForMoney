import React from 'react';
import './RankingList.css';

const RankingList = (props) =>{
  let setRankingCategory = props.setRankinkCategory;
  let rank = 0;
  let rankSkip = 1;
  let prevPoints = 0;
   return (
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
  )
}

export default RankingList;
