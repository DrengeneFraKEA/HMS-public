import React, { Component } from 'react';
import axios from 'axios';
import Navbar from './Navbar';
import { v4 as uuidv4 } from 'uuid';
import '../styling/navbar.css';

export class Rating extends Component {
    constructor(props) {
        super(props)

        this.state = {
            ratings: [],
            newRating: {
                rating: { id: '' },
                doctorName: '',
                title: '',
                text: '',
                score: '',
                uuid: ''
            }
        };
    }

    componentDidMount() {
        this.fetchRatings();
    }

    fetchRatings = async () => {
        try {
            const response = await axios.get("http://localhost:8090/api/rating");
            this.setState({
                ratings: response.data,
            });
        }
        catch (error) {
            console.error('Error fetching ratings: ', error);
        }
    }

    handleInputChange = (e) => {
        this.setState({
            newRating: {
                ...this.state.newRating,
                [e.target.name]: e.target.value
            }
        });
    }

    handleSubmit = async (e) => {
        e.preventDefault();
        try {

            const { doctorName, title, text, score } = this.state.newRating;

            const newRatingWithUUID = {
                uuid: uuidv4(),
                doctorName,
                title,
                text,
                score
            };
            await axios.post("http://localhost:8090/api/rating/create", newRatingWithUUID);

            this.setState({
                newRating: {
                    rating: { id: '' },
                    doctorName: '',
                    title: '',
                    text: '',
                    score: '',
                    uuid: ''
                }
            });
            this.fetchRatings();
        } catch (error) {
            console.error('Error creating rating:', error);
        }
    }

    handleUpdate = async (ratingId) => {
        try {
            const { doctorName, title, text, score } = this.state.newRating;

            const newRatingWithUUID = {
                uuid: uuidv4(),
                rating: { id: ratingId },
                doctorName,
                title,
                text,
                score
            };
            await axios.post("http://localhost:8090/api/rating/create", newRatingWithUUID);

            this.setState({
                newRating: {
                    rating: { id: '' },
                    doctorName: '',
                    title: '',
                    text: '',
                    score: '',
                    uuid: ''
                }
            });
            this.fetchRatings();


        } catch (error) {
            console.error('Error updating rating:', error);
        }
    }

    handleDeleteRating = async (ratingId) => {
        try {
            await axios.post(`http://localhost:8090/api/rating/${ratingId}/delete`);
            this.fetchRatings();
        } catch (error) {
            console.error('Error deleting rating:', error);
        }
    }

    render() {
        const { ratings, newRating } = this.state;

        return (
            <>
                <Navbar />
                <div className="journal-container">
                    <h2>Ratings</h2>
                    <form onSubmit={this.handleSubmit}>
                        <div>
                            <input type="text" name="doctorName" value={newRating.doctorName} onChange={this.handleInputChange} placeholder="Indtast lægens navn" />
                        </div>
                        <div>
                            <input type="text" name="title" value={newRating.title} onChange={this.handleInputChange} placeholder="Skriv en titel" />
                        </div>
                        <div>
                            <textarea name="text" value={newRating.text} onChange={this.handleInputChange} placeholder="Skriv en kommentar" />
                        </div>
                        <div>
                            <input type="text" name="score" value={newRating.score} onChange={this.handleInputChange} placeholder="Indtast en score" />
                        </div>
                        <button type="submit">Submit Rating</button>
                    </form>
                    <ul>
                        {ratings.map((rating) => (
                            <li key={rating.id}>
                                <div className="journal-note">{rating.title}</div>
                                <div className="journal-date">Text: {rating.text}</div>
                                <div className="journal-date">Score: {rating.score}</div>
                                <div className="journal-date">Date: {rating.modifiedDate}</div>
                                <div className="journal-date">Doctor Name: {rating.doctorName}</div>
                                <div className="journal-date">Rating ID: {rating.rating.id}</div>
                                <button onClick={() => this.handleUpdate(rating.rating.id)}>Update</button>
                                <button onClick={() => this.handleDeleteRating(rating.rating.id)}>Delete</button>
                            </li>
                        ))}
                    </ul>
                </div>
            </>
        );
    }
}
