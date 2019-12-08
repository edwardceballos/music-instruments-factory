import React from 'react';
import Model from '../../../domain/Model';
import Mark from '../../../domain/Mark';
import Country from '../../../domain/Country';
import HttpMethod from '../../../util/http/HttpMethods';
import { Link } from 'react-router-dom';

interface ModelEditProps {
    match: { params: { id: any; }; };
}

interface ModelEditState {
    id: number;
    model: Model;
    mark: Array<Mark>;
}

export default class ModelEdit extends React.Component<ModelEditProps, ModelEditState> {

    constructor(props: ModelEditProps) {
        super(props);
        this.state = {
            id: props.match.params.id,
            model: new Model(0, '', new Mark(0, '', new Country(0, '')), new Date()),
            mark: new Array(),
        };
    }

    componentDidMount() {
        let xhr = new XMLHttpRequest();
        xhr.open(HttpMethod.GET, `http://localhost/api/v1/Model/${this.props.match.params.id}`);
        xhr.onload = (evt) => {
            let res: Model = JSON.parse(xhr.responseText);
            this.setState({ model: res });
        };
        xhr.onerror = (evt) => {
            alert("error");
        };
        xhr.send();

        let xhrM = new XMLHttpRequest();
        xhrM.open(HttpMethod.GET, 'http://localhost/api/v1/Mark/');
        xhrM.onload = (evt) => {
            let res: Array<Mark> = JSON.parse(xhrM.responseText);
            this.setState({ mark: res });
        };
        xhrM.onerror = (evt) => {
            alert("error");
        };
        xhrM.send();
    }

    public render() {
        let that: ModelEdit = this;
        return (
            <div className="ModelEdit">
                <form className="form-add">
                    <label>
                        <span>Название модели</span>
                        <input type="text" value={that.state.model != null ? that.state.model.modelName : ''}></input>
                    </label>
                    <label>
                        <span>Марка</span>
                        <select value={this.state.model.mark.name}>
                            {this.state.mark.map((team) => <option key={team.name} value={team.name}>{team.name}</option>)}
                        </select>
                    </label>
                    <label>
                        <span>Год</span>
                        <input type="text" value={that.state.model != null ? that.state.model.year.toString() : ''}></input>
                    </label>
                </form>
                <button className="btn-content">Save</button>
                <Link to="/index/Model"><button className="btn-content">Cancel</button></Link>
            </div>
        );
    }
}