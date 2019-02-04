import React from 'react';
import './RankingList.css';
//import loader from 'img/loader.gif';

const RankingList = (props) =>{
  let rank = 0;
  let rankSkip = 1;
  let prevPoints = 0;
  let rankingList = props.ranking.map( (i, index)=> {
    if(i.points === prevPoints){
      rankSkip++;
    } else {
      rank = rank + rankSkip;
      rankSkip = 1;
    }
    prevPoints = i.points;
    i.rank = rank;
    return(i);
  });

  let substring = (typeof props.rankingInputFilter !== 'undefined' ? props.rankingInputFilter.toLowerCase() : '');

  return (
    <ol className="RankingList">
      {
        rankingList.map( (i, index)=> {
          if((i.athleteFirstName+i.athleteLastName).toLowerCase().includes(substring)){
            return(
              <li key={i.athleteId} className={`RankingListItem ${i.rank === 1 ? 'leader' : ''}`}>

                <div className="RankingListItem-rank">{i.rank}</div>
                <div className="RankingListItem-avatar">
                  <img src={i.profilePictureUrl} alt="" className="RankingListItem-image"/>
                </div>
                <div className="RankingListItem-name">{`${i.athleteFirstName} ${i.athleteLastName}`}</div>
                <div className="RankingListItem-points">{i.points}</div>
              </li>
            );
          }
        })
      }
    </ol>
  )
}

export default RankingList;
