import React, { Component } from 'react';

import './InstaGallery.css';

class InstaGallery extends Component {
  constructor(props) {
    super(props);

    this.state = {
      instaImgUrl: null
    };
  }
  render() {
    let hashtag;
    if(this.state.instaImgUrl){
      hashtag = <p className="InstaGallery__hashtag">#bfmMW</p>;
    }
    return (
      <div className="InstaGallery">
        <div className="InstaGallery__frame">{this.state.instaImgUrl}</div>
        {hashtag}
      </div>

    );
  }
  componentDidMount() {
    fetch("https://www.instagram.com/explore/tags/bfmmw/?__a=1")
      .then(res => res.json())
      .then(
        (result) => {
          function getInstaImgUrl(i){
            return <div className="InstaGallery__image" style={{backgroundImage:`url(${i.node.display_url})`}} key={i.node.id}></div>
          }
          var instaData = result.graphql.hashtag.edge_hashtag_to_top_posts.edges;
          var instaImgUrl = instaData.map(getInstaImgUrl)
          this.setState({
            instaImgUrl: instaImgUrl
          });
        },
        (error) => {
          this.setState({
            instaImgUrl: null,
            error
          });
        }
      )
  }

}

export default InstaGallery;
