import { api } from '../../internal/api';


// Ranking

export function getRanking() {

    return api.get('api/ranking')
        .then(res => res);
}

export function getRankingByDate(month, year) {

    return api.get(`api/ranking?month=${month}&year=${year}`)
        .then(res => res);
      
}

export function getCategoryRanking(category) {

    return api.get('api/ranking/' + category)
        .then(res => res);
}




