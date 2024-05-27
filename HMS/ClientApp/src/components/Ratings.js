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
            },
            submissionMessage: ''
        };
    }
    uuidG = '';
    componentDidMount() {
        this.uuidG = uuidv4();
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
            this.setState({
                submissionMessage: 'Kan ikke oprette forbindelse til Rating'
            });
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
                uuid: this.uuidG,
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
                },
                submissionMessage: 'Rating sendt!'
            });
            this.fetchRatings();
        } catch (error) {
            console.error('Error creating rating:', error);
            this.setState({
                submissionMessage: 'Rating indsendelse fejlede!'
            });
        }
    }

    handleUpdate = async (ratingId) => {
        try {
            const { doctorName, title, text, score } = this.state.newRating;

            const newRatingWithUUID = {
                uuid: this.uuidG,
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
                },
                submissionMessage: 'Rating opdateret'
            });
            this.fetchRatings();


        } catch (error) {
            console.error('Error updating rating:', error);
            this.setState({
                submissionMessage: 'Rating opdatering fejlede.'
            });
        }
    }

    handleDeleteRating = async (ratingId) => {
        try {
            await axios.post(`http://localhost:8090/api/rating/${ratingId}/delete`);
            this.setState({
                submissionMessage: 'Rating slettet!'
            });
            this.fetchRatings();
        } catch (error) {
            console.error('Error deleting rating:', error);
            this.setState({
                submissionMessage: 'Sletning af rating fejlede.'
            });
        }
    }

    render() {
        const { ratings, newRating, submissionMessage } = this.state;

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
                        <div>{submissionMessage}</div>
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