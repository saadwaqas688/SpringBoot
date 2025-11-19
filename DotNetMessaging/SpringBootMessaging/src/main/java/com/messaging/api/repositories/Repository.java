package com.messaging.api.repositories;

import com.messaging.api.models.BaseEntity;
import lombok.RequiredArgsConstructor;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.mongodb.core.MongoTemplate;
import org.springframework.data.mongodb.core.aggregation.AggregationResults;
import org.springframework.data.mongodb.core.query.Criteria;
import org.springframework.data.mongodb.core.query.Query;
import org.springframework.data.mongodb.repository.MongoRepository;

import java.util.List;
import java.util.Optional;

@RequiredArgsConstructor
public abstract class Repository<T extends BaseEntity> implements IRepository<T> {
    
    protected final MongoTemplate mongoTemplate;
    protected final MongoRepository<T, String> mongoRepository;
    protected final Class<T> entityClass;
    
    protected Repository(MongoTemplate mongoTemplate, MongoRepository<T, String> mongoRepository, Class<T> entityClass) {
        this.mongoTemplate = mongoTemplate;
        this.mongoRepository = mongoRepository;
        this.entityClass = entityClass;
    }
    
    @Override
    public T save(T entity) {
        return mongoRepository.save(entity);
    }
    
    @Override
    public Optional<T> findById(String id) {
        return mongoRepository.findById(id);
    }
    
    @Override
    public List<T> findAll() {
        return mongoRepository.findAll();
    }
    
    @Override
    public Page<T> findAll(Pageable pageable) {
        return mongoRepository.findAll(pageable);
    }
    
    @Override
    public List<T> findByField(String fieldName, Object value) {
        Query query = new Query(Criteria.where(fieldName).is(value));
        return mongoTemplate.find(query, entityClass);
    }
    
    @Override
    public void deleteById(String id) {
        mongoRepository.deleteById(id);
    }
    
    @Override
    public void delete(T entity) {
        mongoRepository.delete(entity);
    }
    
    @Override
    public long count() {
        return mongoRepository.count();
    }
    
    @Override
    public boolean existsById(String id) {
        return mongoRepository.existsById(id);
    }
    
    @Override
    public <O> AggregationResults<O> aggregate(org.springframework.data.mongodb.core.aggregation.Aggregation aggregation, Class<O> outputType) {
        return mongoTemplate.aggregate(aggregation, mongoTemplate.getCollectionName(entityClass), outputType);
    }
}

