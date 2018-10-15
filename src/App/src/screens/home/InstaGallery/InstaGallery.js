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
    let hashtag, galleryItems;

    if(this.state.instaImgUrl){
      hashtag = <p className="InstaGallery__hashtag"><a href="https://www.instagram.com/explore/tags/bfmmw/" className="InstaGallery__hashtag-link" target="_blank" rel="noopener noreferrer">#bfmMW</a></p>;
      galleryItems = this.state.instaImgUrl.map(i => <div className="InstaGallery__image" key={i.key} ><img src={i.url} alt='' /></div>) ;
    }

    return (
      <div className="InstaGallery">
        <div className="InstaGallery__frame">
          {galleryItems}
        </div>
        {hashtag}
      </div>

    );
  }
  componentDidMount() {
    fetch("https://www.instagram.com/explore/tags/bfmMW/?__a=1")
      .then(res => res.json())
      .then(
        (result) => {
          let instaData = result.graphql.hashtag.edge_hashtag_to_media.edges;
          let instaImgUrl = instaData.slice(0,6).map(i => {return {url: i.node.display_url, key: i.node.id}; }); //crop data to first 6 results, then save image's urls and ID

          this.setState({
            instaImgUrl: instaImgUrl
          });
        },
        (error) => {
          this.setState({
            instaImgUrl: null,
          });
          console.error('Error:', error);
        }
      )
  }
}

export default InstaGallery;
